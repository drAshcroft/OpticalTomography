function [allSlices, block] = LoadInterpolateImage2D(nRotations, nSlices)


origImage =   imread('image0000.tif');
block=zeros([size(origImage,1) size(origImage,1) 364]);
for I=0:363
    filename =  sprintf('image%04d.tif',I);
    origImage =   imread(filename);
    block(:,:,I+1)=origImage;
end
block(block<100)=0;

allSlices=cell([ nRotations *nSlices 1]);

anglestep = 360/nRotations;

cc=1;

for I=1:nRotations
    tempSlices = cell([nSlices 1]);
    seeds = randi(size(origImage,1),[1 nSlices]);
    angle = anglestep*(I-1);
    for J=1:nSlices
        slice=zeros([size(origImage,1) size(block,3)]);
        t=struct('angle',angle,'offset',0,'array',slice);
        tempSlices{J}=t;
    end
    
    for J=1:size(block,3)
        origImage = block(:,:,J);
        %rotate the image around to get a smooth line
        tempimage = imrotate(origImage,angle,'bilinear','crop');
        
        for K=1:length(seeds)
            chunk = tempimage(seeds(K),:);
           % ss=sum(chunk);
           ss=chunk;
            ss(end)=angle;
            ss(end-1)=seeds(K);
            %ss(end-2)=y;
            slice = tempSlices{K}.array;
            slice(:,J)=ss;
            tempSlices{K}.array=slice;
        end
    end
    
    for J=1:nSlices
        allSlices{cc}=tempSlices{J};
        cc=cc+1;
    end
end

for I=1:length(allSlices)
   imagestruct=allSlices{I};
    filename =  sprintf('C:\\Users\\bashc\\Documents\\MATLAB\\Interpolation2\\slices2\\image%04d.mat',I);
   save(filename,'imagestruct');
end

end