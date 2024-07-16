
nProjections =length(origImages);

step=(length(origImages))/nProjections;

angleStep=360.0/(nProjections+1);

s = size(origImages{2});


sinoGram = zeros([s(2) nProjections]);
for I=1:nProjections
    sI=round( (I-1)*step+1);
    projection = origImages{sI};
    projection = projection(:,110)';
    sinoGram(:,I)=projection(:);
end

thetas = ((1:nProjections)-1)*angleStep;

reconImage=iradon(sinoGram,thetas,'linear','Ram-Lak',1,s(2));

figure;imagesc(reconImage(70:end-70,70:end-70));colormap gray;
D=reconImage;


