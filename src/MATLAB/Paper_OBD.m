NET.addAssembly('C:\Development\CellCT\Tomographic_Imaging - Current\Tomography Simplified_New\bin\DebugDLL\ReconstructCells.dll');

Directories = ReconstructCells.Tools.MatlabHelps.GetPaperDirectories('V:\\ASU_Recon\\viveks_RT10','brightimages')


s=100;
origPSF=zeros([s s]);
for I=1:s/2-1
    radius = I;
    
    padded =zeros([s s]);
    for J=1:s
        for K=1:s
            R=((J-s/2)^2+(K-s/2)^2)^.5;
            if R<radius
                padded(J,K)=1;
            end
        end
    end
    origPSF=origPSF+padded/I^2;
end
h = fspecial('gaussian', 35, 5);
origPSF2=conv2(origPSF,h,'same');


% origPSF2=origPSF;
sumf = sum(origPSF2(:));
origPSF2 = origPSF2/sumf;

for dirI=1:Directories.Length
    dir =char( Directories(dirI));
    
    lib = ReconstructCells.Tools.MatlabHelps.GetLibraryForMatlab([dir '\\brightimages']);
    psf=[];
    tpsf=[];
    for denoise=0:1
        for mIter=3:3
            for pIter=6:4:10
                for bb=0:500:500
                    for opposing=0:4:4
                        try
                            
                            outdir = [dir '\\OBD_p_' num2str(pIter) '_m_' num2str(mIter) '_sig_3_de_' num2str(denoise) '_mod_t_reg_t_op' num2str(opposing) '_bk_' num2str(bb) '_L'];
                            iters=[pIter mIter];
                            % denoise=true;
                            model =true;
                            
                            regularize=true;
                            %  opposing=true;
                            try
                                mkdir(outdir);
                            catch mex
                                
                            end
                            
                            
                            
                            count = lib.Count;
                            for I=1:lib.Count
                                
                                I1=I;
                                %                 I2=mod( I,count)+1;
                                I3=mod( I+250-1,count)+1;
                                %                 I4=mod( I+251-1,count)+1;
                                
                                h0 = double( lib.Item(I1-1).Data);
                                %                 h1 =  double( lib.Item(I2-1).Data);
                                h2 = flipud(  double( lib.Item(I3-1).Data));
                                %                 h3 = flipud( double( lib.Item(I4-1).Data));
                                %
                                %                 back =( mean(h2(1,:))+mean(h2(:,1)))/2;
                                %                 h2=back - h2;
                                
                                h0=h0(10:end-10,10:end-10);
                                back =( mean(h0(1,:))+mean(h0(:,1)))/2;
                                h0=back - h0+bb;
                                
                                h2=h2(10:end-10,10:end-10);
                                back =( mean(h2(1,:))+mean(h2(:,1)))/2;
                                h2=back - h2+bb;
                                
                                if opposing==4
                                    y={h0 h0 };
                                else
                                    if opposing==0
                                        y={h0 h0 h0};
                                    else
                                        if opposing ==1
                                            y={h0 h2 };
                                        else
                                            y={h0 h2 h0};
                                        end
                                    end
                                end
                                
                                psf=[];
                                [image ] = DeconvolveImageOBD2(y,origPSF2,true, 1.0, denoise ,model, iters,regularize );
                                
                                image2=NET.convertArray(image,'System.Single',size(image));
                                
                                ReconstructCells.Tools.MatlabHelps.SaveTiff(outdir,I-1,image2);
                                
                            end
                            
                        catch mex
                            disp(mex)
                            
                        end
                    end
                end
            end
        end
    end
end